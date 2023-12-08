// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

import Foundation
import WatchKit
import WatchConnectivity
import CoreMotion
import HealthKit
import simd

enum MofaView: Int {
    case readyView = 0
    case handednessView = 1
    case fightingView = 2
    case resultView = 3
}

enum MofaRoundResult: Int {
    case victory = 0
    case defeat = 1
    case draw = 2
}

enum MofaMagicSchool: Int {
    case mysticArt = 0
    case thunder = 1
    case harryPotter = 2
    case jLaser = 4
    case aceFire = 5
    case wind = 6
    case bathToy = 7
    case electric = 8
    case water = 9
    case timeStone = 10
}

enum MofaWatchState: Int {
    case normal = 0
    case ground = 1
}

enum MofaHandedness: Int {
    case right = 0
    case left = 1
}

class MofaWatchAppManager: NSObject, ObservableObject {
    
    @Published var view: MofaView = .readyView
    
    @Published var magicSchool: MofaMagicSchool = .mysticArt
    
    @Published var handedness: MofaHandedness = .right {
        didSet {
            UserDefaults.standard.set(handedness.rawValue, forKey: handednessKey)
        }
    }
    
    // Round result stats
    @Published var roundResult: MofaRoundResult = .victory
    @Published var kill: Int = 0
    // 0.1 means 10% hit rate
    @Published var hitRate: Float = 0
    
    let motionManager = CMMotionManager()
    
    let healthStore = HKHealthStore()
    var workoutSession: HKWorkoutSession?
    var builder: HKLiveWorkoutBuilder?
    
    let deviceMotionUpdateInterval: Double = 0.016
    var currentState: MofaWatchState = .normal
    let sharedInputCd: Double = 0.5
    var lastInputTime: Double = 0
    // This vector varies with handedness and digital crown orientation
    var groundVector = simd_double3(-1, 0, 0)
    var lastStartRoundTime: Double = 0
    
    let meterToFoot: Double = 3.2808
    let handednessKey: String = "UserHandedness"
    
    override init() {
        super.init()
        requestHealthKitAuthorization()
        if UserDefaults.standard.object(forKey: handednessKey) != nil  {
            self.handedness = MofaHandedness(rawValue: UserDefaults.standard.integer(forKey: handednessKey)) ?? .right
        }
    }
    
    func requestHealthKitAuthorization() {
        let typesToShare: Set = [ HKQuantityType.workoutType() ]
        
        let typesToRead: Set = [
            HKQuantityType.quantityType(forIdentifier: .heartRate)!,
            HKQuantityType.quantityType(forIdentifier: .activeEnergyBurned)!,
            HKQuantityType.quantityType(forIdentifier: .distanceWalkingRunning)!,
            HKObjectType.activitySummaryType()
        ]
        
        // Request authorization for those quantity types.
        healthStore.requestAuthorization(toShare: typesToShare, read: typesToRead) { (success, error) in
            if error != nil {
                print("Got error when requesting HealthKit authorization: \(String(describing: error))")
                return
            }
            if success {
                //print("HealthKit authorization requested")
            } else {
                print("Falied to request HealthKit authorization")
            }
        }
    }
    
    // This function is called when watch app switched to MOFA panel
    func onAppear() {
        self.view = .readyView
    }
    
    func OnDisappear() {
        stopRound()
    }
    
    public func startWorkout() {
        if (self.workoutSession?.state == .running) { return }
        
        resetWorkout()
        
        let configuration = HKWorkoutConfiguration()
        configuration.activityType = .play
        configuration.locationType = .indoor
        
        // Create the session and obtain the workout builder
        do {
            workoutSession = try HKWorkoutSession(healthStore: healthStore, configuration: configuration)
            builder = workoutSession?.associatedWorkoutBuilder()
        } catch {
            print("Failed to create workout session")
            return
        }
        
        workoutSession?.delegate = self
        builder?.delegate = self
        
        // Set the workout builder's data source
        builder?.dataSource = HKLiveWorkoutDataSource(healthStore: healthStore, workoutConfiguration: configuration)
        
        let startDate = Date()
        workoutSession?.startActivity(with: startDate)
        builder?.beginCollection(withStart: startDate) { (success, error) in
            // The workout has started
        }
    }
    
    public func endWorkout() {
        workoutSession?.end()
        workoutSession = nil
        sendHealthDataMessage()
    }
    
    public func startCoreMotion() {
        // Check if we can start core motion now
        if (motionManager.isDeviceMotionAvailable && !motionManager.isDeviceMotionActive) {
            motionManager.deviceMotionUpdateInterval = self.deviceMotionUpdateInterval
            // Check handedness
            if (self.handedness == .right) {
                if (WKInterfaceDevice.current().crownOrientation == .right) {
                    self.groundVector = simd_double3(-1, 0, 0)
                } else {
                    self.groundVector = simd_double3(1, 0, 0)
                }
            }
            else {
                if (WKInterfaceDevice.current().crownOrientation == .right) {
                    self.groundVector = simd_double3(1, 0, 0)
                } else {
                    self.groundVector = simd_double3(-1, 0, 0)
                }
            }
            motionManager.startDeviceMotionUpdates(using: .xMagneticNorthZVertical, to: OperationQueue.current!) { (data: CMDeviceMotion?, error: Error?) in
                if error != nil {
                    return
                }
                
                let currentTime = ProcessInfo.processInfo.systemUptime
                
                guard let acceleration: CMAcceleration = data?.userAcceleration else {
                    return
                }
                guard let gravity = data?.gravity else {
                    return
                }
                let gravityVector3 = simd_double3(gravity.x, gravity.y, gravity.z)
                
                if (currentTime - self.lastInputTime > self.sharedInputCd) {
                    if (abs(acceleration.x) > 1.6) {
                        print("Triggered")
                        self.sendWatchTriggeredMessage()
                        self.lastInputTime = currentTime
                        return
                    }
                }
                
                if (simd_dot(gravityVector3, self.groundVector) > 0.7) {
                    if (self.currentState != .ground) {
                        print("Changed to ground")
                        self.currentState = .ground
                        self.sendWatchStateChangedMessage(watchState: self.currentState)
                        return
                    }
                } else {
                    if (self.currentState != .normal) {
                        print("Changed to normal")
                        self.currentState = .normal
                        self.sendWatchStateChangedMessage(watchState: self.currentState)
                        return
                    }
                }
            }
        }
    }
    
    public func endCoreMotion() {
        if (motionManager.isDeviceMotionAvailable && motionManager.isDeviceMotionActive) {
            motionManager.stopDeviceMotionUpdates()
        }
    }
    
    public func startRound() {
        startWorkout()
        startCoreMotion()
        self.view = .fightingView
    }
    
    public func stopRound() {
        endCoreMotion()
        endWorkout()
        self.view = .resultView
    }
    
    func sendStartRoundMessage() {
        let message = ["MOFA" : true,
                       "Start": true]
        HoloKitWatchAppManager.shared.wcSession.sendMessage(message, replyHandler: nil)
        startWorkout()
    }
    
    func sendWatchTriggeredMessage() {
        let message = ["MOFA" : true,
                       "WatchTriggered" : true]
        HoloKitWatchAppManager.shared.wcSession.sendMessage(message, replyHandler: nil)
    }
    
    func sendWatchStateChangedMessage(watchState: MofaWatchState) {
        let message = ["MOFA" : true,
                       "WatchState" : watchState.rawValue] as [String : Any]
        HoloKitWatchAppManager.shared.wcSession.sendMessage(message, replyHandler: nil)
    }
    
    func sendHealthDataMessage() {
        let dist = Float(self.distance)
        let energy = Float(self.activeEnergy)
        let message = ["MOFA" : true,
                       "Dist" : dist,
                       "Energy" : energy] as [String : Any]
        HoloKitWatchAppManager.shared.wcSession.sendMessage(message, replyHandler: nil)
    }
    
    // MARK: - Workout Metrics
    @Published var averageHeartRate: Double = 0
    @Published var heartRate: Double = 0
    @Published var activeEnergy: Double = 0
    @Published var distance: Double = 0
    @Published var workout: HKWorkout?
    
    func updateForStatistics(_ statistics: HKStatistics?) {
        guard let statistics = statistics else { return }
        
        DispatchQueue.main.async {
            switch statistics.quantityType {
            case HKQuantityType.quantityType(forIdentifier: .heartRate):
                let heartRateUnit = HKUnit.count().unitDivided(by: HKUnit.minute())
                self.heartRate = statistics.mostRecentQuantity()?.doubleValue(for: heartRateUnit) ?? 0
                self.averageHeartRate = statistics.averageQuantity()?.doubleValue(for: heartRateUnit) ?? 0
            case HKQuantityType.quantityType(forIdentifier: .activeEnergyBurned):
                let energyUnit = HKUnit.kilocalorie()
                self.activeEnergy = statistics.sumQuantity()?.doubleValue(for: energyUnit) ?? 0
            case HKQuantityType.quantityType(forIdentifier: .distanceWalkingRunning), HKQuantityType.quantityType(forIdentifier: .distanceCycling):
                let meterUnit = HKUnit.meter()
                self.distance = statistics.sumQuantity()?.doubleValue(for: meterUnit) ?? 0
            default:
                return
            }
        }
    }
    
    func resetWorkout() {
        builder = nil
        workout = nil
        workoutSession = nil
        activeEnergy = 0
        averageHeartRate = 0
        heartRate = 0
        distance = 0
    }
}

// MARK: - Mock WCSessionDelegate
extension MofaWatchAppManager {
    func didReceiveApplicationContext(applicationContext: [String : Any]) {
        if applicationContext["Start"] is Bool {
            if let magicSchool = applicationContext["MagicSchool"] as? Int {
                DispatchQueue.main.async {
                    self.magicSchool = MofaMagicSchool(rawValue: magicSchool)!
                }
            }
            
            if (self.view != .fightingView) {
                print("MOFA round started")
                DispatchQueue.main.async {
                    self.startRound()
                }
            }
            return
        }
        
        if applicationContext["End"] is Bool {
            if (self.view == .fightingView) {
                print("MOFA round ended")
                if let roundResultIndex = applicationContext["Result"] as? Int {
                    if let roundResult = MofaRoundResult(rawValue: roundResultIndex) {
                        DispatchQueue.main.async {
                            self.roundResult = roundResult
                        }
                    }
                }
                if let kill = applicationContext["Kill"] as? Int {
                    DispatchQueue.main.async {
                        self.kill = kill
                    }
                }
                if let hitRate = applicationContext["HitRate"] as? Float {
                    DispatchQueue.main.async {
                        self.hitRate = hitRate
                    }
                }
                DispatchQueue.main.async {
                    self.stopRound()
                }
            }
            return
        }
    }
    
    func didReceiveMessage(message: [String : Any]) {
        
    }
    
    func didReceiveMessage(message: [String : Any], replyHandler: @escaping ([String : Any]) -> Void) {
        if message["QueryWatchState"] is Int {
            let replyMessage = ["WatchState" : self.currentState.rawValue];
            replyHandler(replyMessage)
        }
    }
}

// MARK: - HKWorkoutSessionDelegate
extension MofaWatchAppManager: HKWorkoutSessionDelegate {
    func workoutSession(_ workoutSession: HKWorkoutSession, didChangeTo toState: HKWorkoutSessionState, from fromState: HKWorkoutSessionState, date: Date) {
        //print("workoutSessionDidChangeTo \(toState) from \(fromState)")
        DispatchQueue.main.async {
            
        }
        
        // Wait for the session to transition states before ending the builder.
        if toState == .ended {
            builder?.endCollection(withEnd: date) { (success, error) in
                self.builder?.finishWorkout { (workout, error) in
                    DispatchQueue.main.async {
                        self.workout = workout
                    }
                }
            }
        }
    }
    
    func workoutSession(_ workoutSession: HKWorkoutSession, didFailWithError error: Error) {
        
    }
}

// MARK: - HKLiveWorkoutBuilderDelegate
extension MofaWatchAppManager: HKLiveWorkoutBuilderDelegate {
    func workoutBuilderDidCollectEvent(_ workoutBuilder: HKLiveWorkoutBuilder) {
        //print("event: \(String(describing: workoutBuilder.workoutEvents.last?.type))")
    }
    
    func workoutBuilder(_ workoutBuilder: HKLiveWorkoutBuilder, didCollectDataOf collectedTypes: Set<HKSampleType>) {
        //print("workoutBuilder didCollectDataOf")
        for type in collectedTypes {
            guard let quantityType = type as? HKQuantityType else {
                return
            }
            
            let statistics = workoutBuilder.statistics(for: quantityType)
            
            // Update the published values.
            updateForStatistics(statistics)
        }
    }
}

// MARK: - Math
extension MofaWatchAppManager {
    func rad2deg(_ number: Double) -> Double {
        return number * 180 / .pi
    }
    
    func sameSign(_ num1: Double, _ num2: Double) -> Bool {
        return num1 >= 0 && num2 >= 0 || num1 < 0 && num2 < 0
    }
}
