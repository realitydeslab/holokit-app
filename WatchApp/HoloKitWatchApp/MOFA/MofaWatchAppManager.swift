import Foundation
import WatchKit
import WatchConnectivity
import CoreMotion
import HealthKit
import simd

enum WatchState: Int {
    case nothing = 0
    case sky = 1
    case ground = 2
}

enum WatchInput: Int {
    case castSpell = 0
    case changeToNothing = 1
    case changeToGround = 2
}

enum MofaView: Int {
    case introView = 0
    case handednessView = 1
    case fightingView = 2
    case resultView = 3
}

enum MofaRoundResult: Int {
    case victory = 0
    case defeat = 1
    case draw = 2
}

class MofaWatchAppManager: NSObject, ObservableObject {
    
    @Published var currentView: MofaView = .introView
    
    @Published var isRightHand: Bool = true
    
    @Published var rememberHandedness: Bool = false
    
    @Published var isFighting: Bool = false
    
    @Published var roundResult: MofaRoundResult = .victory
    
//    @Published var showingSummaryView: Bool = false {
//        didSet {
//            if showingSummaryView == false {
//                resetWorkout()
//            }
//        }
//    }
     
    let motionManager = CMMotionManager()

    let healthStore = HKHealthStore()
    var wcSession: WCSession!
    var workoutSession: HKWorkoutSession?
    var builder: HKLiveWorkoutBuilder?
    
    var currentState: WatchState = .nothing
    let sharedInputCd: Double = 0.5
    var lastInputTime: Double = 0
    
    override init() {
        super.init()
        
        requestHealthKitAuthorization()
    }
    
    public func InitializeMofaWCSessionDelegate() {
        if (WCSession.isSupported()) {
            wcSession = WCSession.default
            wcSession.delegate = self
        }
    }
    
    func requestHealthKitAuthorization() {
        let typesToShare: Set = [HKQuantityType.workoutType()]
        
        let typesToRead: Set = [
            HKQuantityType.quantityType(forIdentifier: .heartRate)!,
            HKQuantityType.quantityType(forIdentifier: .activeEnergyBurned)!,
            HKQuantityType.quantityType(forIdentifier: .distanceWalkingRunning)!,
            HKObjectType.activitySummaryType()
        ]
        
        // Request authorization for those quantity types.
        healthStore.requestAuthorization(toShare: typesToShare, read: typesToRead) { (success, error) in
            if error != nil {
                print("Error when requesting HealthKit authorization: \(String(describing: error))")
                return
            }
            if success {
                print("Successfully requested HealthKit authorization")
            } else {
                print("Falied to request HealthKit authorization")
            }
        }
    }
    
    func SendStartRoundMessage() {
        let readyMessage = ["StartRound": 0];
        self.wcSession.sendMessage(readyMessage, replyHandler: nil, errorHandler: { error in
            print("Failed to send message to iPhone with error: \(error.localizedDescription)")
        })
    }
    
    public func startRound() {
        SendStartRoundMessage()
        startCoreMotion()
        startWorkout()
        self.isFighting = true
    }
    
    public func stopRound() {
        endCoreMotion()
        endWorkout()
        self.isFighting = false
    }
    
    public func startWorkout() {
        let configuration = HKWorkoutConfiguration()
        configuration.activityType = .running
        configuration.locationType = .indoor
        
        do {
            workoutSession = try HKWorkoutSession(healthStore: healthStore, configuration: configuration)
            builder = workoutSession?.associatedWorkoutBuilder()
        } catch {
            print("Failed to create workout session")
            return
        }
        
        workoutSession?.delegate = self
        builder?.delegate = self
        builder?.dataSource = HKLiveWorkoutDataSource(healthStore: healthStore, workoutConfiguration: configuration)
        
        let startDate = Date()
        workoutSession?.startActivity(with: startDate)
        builder?.beginCollection(withStart: startDate) { (success, error) in
            if error != nil {
                print("Error when beginning collect workout data: \(String(describing: error))")
                return
            }
            if success {
                print("Started to collect workout data")
            } else {
                print("Failed to collect workout data")
            }
        }
    }
    
    public func endWorkout() {
        workoutSession?.end()
    }
    
    public func startCoreMotion() {
        motionManager.deviceMotionUpdateInterval = 0.016
        if (motionManager.isDeviceMotionAvailable && !motionManager.isDeviceMotionActive) {
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
                    if (acceleration.x < -1.6) {
                        print("Cast spell")
                        self.sendWatchInputMessage(watchInput: .castSpell)
                        self.lastInputTime = currentTime
                        return
                    }
                }
                
                if (simd_dot(gravityVector3, simd_double3(1, 0, 0)) > 0.7) {
                    if (self.currentState != .sky) {
                        print("changed to sky")
                        self.currentState = .sky
                        //self.sendWatchInputMessage(wathInput: .basic)
                        return
                    }
                } else if (simd_dot(gravityVector3, simd_double3(-1, 0, 0)) > 0.7) {
                    if (self.currentState != .ground) {
                        print("changed to ground")
                        self.currentState = .ground
                        self.sendWatchInputMessage(watchInput: .changeToGround)
                        return
                    }
                } else {
                    if (self.currentState != .nothing) {
                        print("changed to nothing")
                        self.currentState = .nothing
                        self.sendWatchInputMessage(watchInput: .changeToNothing)
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
    
    func sendWatchInputMessage(watchInput: WatchInput) {
        let message = ["WatchInput": watchInput.rawValue]
        self.wcSession.sendMessage(message, replyHandler: nil, errorHandler: nil)
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
            case HKQuantityType.quantityType(forIdentifier: .distanceWalkingRunning):
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

// MARK: - WCSessionDelegate
extension MofaWatchAppManager: WCSessionDelegate {
    func session(_ session: WCSession, activationDidCompleteWith activationState: WCSessionActivationState, error: Error?) {
        
    }
    
    func session(_ session: WCSession, didReceiveMessage message: [String : Any]) {
        if message["QueryWatchState"] is Int {
            print("Received QueryWatchState message");
            switch(self.currentState) {
            case .nothing:
                self.sendWatchInputMessage(watchInput: .changeToNothing)
                break
            case .sky:
                //self.sendWatchInputMessage(watchInput: .change2Sky)
                break
            case .ground:
                self.sendWatchInputMessage(watchInput: .changeToGround)
                break
            }
            return
        }
        
        if message["RoundResult"] is Int {
            let roundResult = message["RoundResult"] as! MofaRoundResult;
            let kill: Int = message["Kill"] as! Int;
            let hitRate: Float = message["HitRate"] as! Float;
            print("RoundResult \(roundResult), Kill \(kill) and HitRate \(hitRate)")
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
