package com.grandlinequotes.api

interface IQuoteService {

    suspend fun listQuotes(authorId: Int?, arcId: Int?, searchTerm: String?): List<ListQuotesQuery.Quote>
    suspend fun getQuote(id: Int): GetQuoteQuery.Quote?
}