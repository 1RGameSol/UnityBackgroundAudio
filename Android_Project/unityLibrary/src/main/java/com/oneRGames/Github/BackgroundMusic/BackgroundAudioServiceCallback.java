package com.oneRGames.Github.BackgroundMusic;

public interface BackgroundAudioServiceCallback {
    void BackgroundAudioStarted();
    void BackgroundAudioStopped();
    void BackgroundAudioPaused();
    void BackgroundAudioResumed();
}
