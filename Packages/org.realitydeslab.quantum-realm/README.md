# Quantum Realm (Quantum Buddhas)

The host reveals a sequence of ten particle Buddhas with a hand; spectators watch.

## Interactions (host)

1. Aim at the floor until the hover-to-start button appears.
2. Start the session: hold a hand on the button for its dwell time, **or** press the Star mode
   Trigger button, **or** in phone mode tap the button on screen. The last two work when hand
   tracking misses.
3. A group of ten Buddhas spawns around the player; one is shown at a time.
4. Hold a hand within 0.7 m of the shown Buddha for 2 s: its particles react to the hand and
   dissolve with the dwell progress, then the next Buddha appears. After the tenth it wraps to the first.
5. Moving the hand away before 2 s cancels the dwell; nothing switches.
6. The Switch button shows the next Buddha without a hand.

Spectators see the host's hand point and the current Buddha. Clients never get the start button
(it is removed on clients); only the host starts the session.

## Automated tests

| Test | What it covers |
| --- | --- |
| `QuantumRealmBuddhaInteractionTests.HostHandDwellActivatesEveryBuddhaInTurn` | Steps 2–6 in Editor Play Mode (XR Simulation), with a synthetic hand pose injected into `HoloKitHandTracker` |
| `QuantumRealmBuddhaInteractionTests.ActivatedBuddhaIsVisibleToThePlayer` | Turns the player toward the shown Buddha and compares renders with and without its effect. **Fails as of 2026-09-28**: the Buddha point cloud draws nothing in the Editor while the seat ring does; not yet checked on a device |
| `QuantumRealmBuddhaVfxTests` | Every Buddha effect exposes the properties its controller drives. "Particles 1" has no `Process` property (as in the App Store build), so the first Buddha shows no dissolve progress |
| `HoverableStartButtonTriggerTests` (shared) | Trigger button and phone-mode tap start a hover button without a hand |
| `Tests/Device` | On an iPhone: synthetic hand motion, hover start and switch |

## Not covered

- Camera-based hand recognition (Vision on the device).
- Multiplayer spectators: no multi-Editor scenario yet.
