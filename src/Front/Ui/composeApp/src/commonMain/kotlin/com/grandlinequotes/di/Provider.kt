package com.grandlinequotes.di

import com.apollographql.apollo.ApolloClient
import com.grandlinequotes.Config
import com.grandlinequotes.api.IQuoteService
import com.grandlinequotes.api.QuoteService
import com.grandlinequotes.app.viewModels.HomeViewModel
import com.grandlinequotes.app.viewModels.QuoteDetailViewModel
import com.grandlinequotes.app.viewModels.QuotesListViewModel
import com.grandlinequotes.network.LanguageInterceptor
import com.grandlinequotes.network.ApolloAuthInterceptor
import co.touchlab.kermit.Logger
import org.koin.core.module.dsl.bind
import org.koin.core.module.dsl.singleOf
import org.koin.dsl.module

val modules = module {

    single {
        ApolloClient
            .Builder()
            .serverUrl(Config.graphqlBaseUrl)
            .addHttpInterceptor(LanguageInterceptor())
            .addHttpInterceptor(ApolloAuthInterceptor())
            .build()
    }

    singleOf(::QuoteService) { bind<IQuoteService>() }

    single { Logger.withTag("GLQuotes") }

    factory { HomeViewModel(get(), get()) }

    factory { QuotesListViewModel(get(), get()) }
    factory { QuoteDetailViewModel(get(), get()) }
}