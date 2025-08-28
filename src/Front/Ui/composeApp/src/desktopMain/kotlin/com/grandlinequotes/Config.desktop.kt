package com.grandlinequotes

actual object Config {
    actual val graphqlBaseUrl: String
        get() = BuildKonfig.API_BASE_URL
    actual val videoBaseUrl: String
        get() = BuildKonfig.VIDEO_BASE_URL
}