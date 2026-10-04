# Quantum Realm (Quantum Buddhas)

The host reveals a sequence of ten particle Buddhas with a hand; spectators watch.

## Interactions (host)

1. Aim at the floor until the hover-to-start button appears; it floats about 0.6 m in front of the player.
2. Start the session: hold a hand on the button for its dwell time (2.4 s), **or** press the Star mode
   Trigger button, **or** in phone mode tap the button on screen. The last two work when hand
   tracking misses.
3. A group of ten Buddhas spawns around the player; one is shown at a time.
4. Touch the shown Buddha for 2 s: the touch point is the index knuckle projected 0.5 m further along
   the eye-to-hand ray, and it must be within 0.7 m of the Buddha. Its particles react to the hand and
   dissolve with the dwell progress, then the next Buddha appears. After the tenth it wraps to the first.
5. Moving the hand away before 2 s cancels the dwell; nothing switches.
6. The Switch button shows the next Buddha without a hand.

Spectators see the host's hand point and the current Buddha. Clients never get the start button
(it is removed on clients); only the host starts the session.

## Automated tests

| Test | What it covers |
| --- | --- |
| `QuantumRealmBuddhaInteractionTests.HostHandDwellActivatesEveryBuddhaInTurn` | Steps 2–6 in Editor Play Mode (XR Simulation), with a synthetic hand pose injected into `HoloKitHandTracker` |
| `QuantumRealmBuddhaInteractionTests.ActivatedBuddhaIsVisibleToThePlayer` | Turns the player toward the shown Buddha and compares renders with and without its effect (passes since 2026-10-04, when the Editor test began rendering while it waits) |
| `QuantumRealmJourneyTests` | End to end with a simulated person in the test arena: scan the floor, start with a hand on the floating button, walk the circle and touch each of the ten Buddhas in turn with the extended touch, checking the player sees each one and only one is shown; the event timeline is compared with `Tests/Editor/Baselines/QuantumRealmJourney.json` |
| `QuantumRealmMultiplayerTests` | Three Editors (`Tools/run_multiplayer_test.sh QRealm QuantumRealmMultiplayerTests`): clients get no start button; while the host dwells on three Buddhas and then presses Switch, a second client and a spectator see the host's hand point reach each Buddha and switch to the same Buddha; the spectator renders its view |
| `QuantumRealmBuddhaVfxTests` | Every Buddha effect exposes the properties its controller drives. "Particles 1" has no `Process` property (as in the App Store build), so the first Buddha shows no dissolve progress |
| `HoverableStartButtonTriggerTests` (shared) | Trigger button and phone-mode tap start a hover button without a hand |
| `Tests/Device` | On an iPhone: synthetic hand motion, hover start and switch |

## Not covered

- Camera-based hand recognition (Vision on the device).
- Multipeer Connectivity and QR alignment (the multi-Editor test uses UnityTransport in one simulated room).
