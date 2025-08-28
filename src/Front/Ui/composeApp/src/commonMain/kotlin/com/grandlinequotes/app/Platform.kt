package com.grandlinequotes.app

interface Platform {
    val name: String
}

expect fun getPlatform(): Platform