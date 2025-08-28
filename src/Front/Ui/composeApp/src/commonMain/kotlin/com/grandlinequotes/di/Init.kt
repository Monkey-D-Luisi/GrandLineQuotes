package com.grandlinequotes.di

import co.touchlab.kermit.Logger
import org.koin.core.context.startKoin
import org.koin.core.error.KoinApplicationAlreadyStartedException
import org.koin.dsl.KoinAppDeclaration

class Init {
    companion object {
        fun initKoin(config: KoinAppDeclaration? = null) {
            try {
                startKoin {
                    config?.invoke(this)
                    modules(modules)
                }
            } catch (e: KoinApplicationAlreadyStartedException) {
                Logger.withTag("Init").w(e) { "Koin application already started" }
            }
        }
    }
}