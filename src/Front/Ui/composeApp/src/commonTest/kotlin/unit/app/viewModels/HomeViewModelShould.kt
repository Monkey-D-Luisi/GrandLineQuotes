package com.grandlinequotes.app.viewModels.unit

import com.grandlinequotes.api.GetQuoteQuery
import com.grandlinequotes.api.IQuoteService
import com.grandlinequotes.api.ListQuotesQuery
import com.grandlinequotes.app.viewModels.HomeViewModel
import co.touchlab.kermit.Logger
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.ExperimentalCoroutinesApi
import kotlinx.coroutines.test.StandardTestDispatcher
import kotlinx.coroutines.test.advanceUntilIdle
import kotlinx.coroutines.test.resetMain
import kotlinx.coroutines.test.runTest
import kotlinx.coroutines.test.setMain
import kotlin.test.AfterTest
import kotlin.test.BeforeTest
import kotlin.test.Test
import kotlin.test.assertFalse
import kotlin.test.assertTrue

@OptIn(ExperimentalCoroutinesApi::class)
class HomeViewModelShould {
    private val dispatcher = StandardTestDispatcher()

    @BeforeTest
    fun setup() {
        Dispatchers.setMain(dispatcher)
    }

    @AfterTest
    fun tearDown() {
        Dispatchers.resetMain()
    }

    @Test
    fun call_service_and_toggle_loading() = runTest {
        val fakeService = object : IQuoteService {
            var called = false
            override suspend fun listQuotes(authorId: Int?, arcId: Int?, searchTerm: String?): List<ListQuotesQuery.Quote> {
                called = true
                return listOf(null as ListQuotesQuery.Quote)
            }
            override suspend fun getQuote(id: Int): GetQuoteQuery.Quote? = null
        }

        val viewModel = HomeViewModel(fakeService, Logger.withTag("test"))

        viewModel.loadRandomQuote()
        advanceUntilIdle()

        assertTrue(fakeService.called, "Service should be called")
        assertFalse(viewModel.isLoading.value, "Loading should be false after load")
    }
}
