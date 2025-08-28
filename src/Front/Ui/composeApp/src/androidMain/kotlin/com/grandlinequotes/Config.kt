package com.grandlinequotes

import com.grandlinequotes.app.BuildConfig

actual object Config {
    actual val apiBaseUrl: String = BuildConfig.API_BASE_URL
    actual val graphqlBaseUrl: String = BuildConfig.GRAPHQL_ENDPOINT
    actual val videoBaseUrl: String = BuildConfig.VIDEO_BASE_URL
}