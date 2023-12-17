// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

#include "cardboard_input_api.h"
#include <array>

extern "C" {

void* HoloKit_LowLatencyTracking_init() {
    cardboard::unity::CardboardInputApi *cardboard_input_api = new cardboard::unity::CardboardInputApi();
    return static_cast<void *>(cardboard_input_api);
}

void HoloKit_LowLatencyTracking_initHeadTracker(void *self) {
    cardboard::unity::CardboardInputApi *cardboard_input_api = static_cast<cardboard::unity::CardboardInputApi *>(self);
    cardboard_input_api->InitHeadTracker();
}

void HoloKit_LowLatencyTracking_pauseHeadTracker(void *self) {
    cardboard::unity::CardboardInputApi *cardboard_input_api = static_cast<cardboard::unity::CardboardInputApi *>(self);
    cardboard_input_api->PauseHeadTracker();
}

void HoloKit_LowLatencyTracking_resumeHeadTracker(void *self) {
    cardboard::unity::CardboardInputApi *cardboard_input_api = static_cast<cardboard::unity::CardboardInputApi *>(self);
    cardboard_input_api->ResumeHeadTracker();
}

void HoloKit_LowLatencyTracking_addSixDoFData(void *self, int64_t timestamp_ns, float *position, float *orientation) {
    cardboard::unity::CardboardInputApi *cardboard_input_api = static_cast<cardboard::unity::CardboardInputApi *>(self);
    cardboard_input_api->AddSixDoFData(timestamp_ns, position, orientation);
}

void HoloKit_LowLatencyTracking_getHeadTrackerPose(void *self, float *position, float *orientation) {
    cardboard::unity::CardboardInputApi *cardboard_input_api = static_cast<cardboard::unity::CardboardInputApi *>(self);
    
    std::array<float, 3> out_position;
    std::array<float, 4> out_orientation;
    cardboard_input_api->GetHeadTrackerPose(out_position.data(), out_orientation.data());
    
    position[0] = out_position.at(0);
    position[1] = out_position.at(1);
    position[2] = -out_position.at(2);
    orientation[0] = out_orientation.at(0);
    orientation[1] = out_orientation.at(1);
    orientation[2] = -out_orientation.at(2);
    orientation[3] = out_orientation.at(3);
}

void HoloKit_LowLatencyTracking_delete(void *self) {
    cardboard::unity::CardboardInputApi *cardboard_input_api = static_cast<cardboard::unity::CardboardInputApi *>(self);
    delete cardboard_input_api;
}

}
