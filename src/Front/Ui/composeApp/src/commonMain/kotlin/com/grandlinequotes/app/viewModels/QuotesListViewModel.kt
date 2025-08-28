package com.grandlinequotes.app.viewModels

import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.setValue
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.grandlinequotes.api.IQuoteService
import com.grandlinequotes.api.ListQuotesQuery
import co.touchlab.kermit.Logger
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.launch

class QuotesListViewModel(
    private val quoteService: IQuoteService,
    private val logger: Logger
) : ViewModel() {

    private val _quotes = MutableStateFlow<List<ListQuotesQuery.Quote>>(emptyList())
    val quotes: StateFlow<List<ListQuotesQuery.Quote>> = _quotes

    private val _authors = MutableStateFlow<List<ListQuotesQuery.Author>>(emptyList())
    val authors: StateFlow<List<ListQuotesQuery.Author>> = _authors

    private val _arcs = MutableStateFlow<List<ListQuotesQuery.Arc>>(emptyList())
    val arcs: StateFlow<List<ListQuotesQuery.Arc>> = _arcs

    var selectedAuthorId by mutableStateOf<Int?>(null)
    var selectedArcId by mutableStateOf<Int?>(null)
    var searchTerm by mutableStateOf<String?>(null)

    private val _isLoading = MutableStateFlow(false)
    val isLoading: StateFlow<Boolean> = _isLoading

    private val _errorMessage = MutableStateFlow<String?>(null)
    val errorMessage: StateFlow<String?> = _errorMessage

    fun clearError() {
        _errorMessage.value = null
    }

    fun loadQuotes(
        authorId: Int? = this.selectedAuthorId,
        arcId: Int? = this.selectedArcId,
        searchTerm: String? = this.searchTerm) {
        viewModelScope.launch {
            _isLoading.value = true
            try {
                val result = quoteService.listQuotes(authorId, arcId, searchTerm)

                _quotes.value = result

                if (_authors.value.isEmpty()) {
                    _authors.value = result
                        .mapNotNull { it.author }
                        .distinctBy { it.id }
                }

                if (_arcs.value.isEmpty()) {
                    _arcs.value = result
                        .mapNotNull { it.episode }
                        .mapNotNull { it.arc }
                        .distinctBy { it.id }
                }
            } catch (e: Exception) {
                logger.e(e) { "Error loading quotes" }
                _errorMessage.value = e.message
            } finally {
                _isLoading.value = false
            }
        }
    }
}