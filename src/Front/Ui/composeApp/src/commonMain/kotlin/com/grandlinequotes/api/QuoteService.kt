package com.grandlinequotes.api

import com.apollographql.apollo.ApolloClient
import com.apollographql.apollo.api.Optional

class QuoteService(private val apolloClient: ApolloClient) : IQuoteService {

    override suspend fun listQuotes(
        authorId: Int?,
        arcId: Int?,
        searchTerm: String?): List<ListQuotesQuery.Quote> {
        val response = apolloClient.query(
            ListQuotesQuery(
                authorId = authorId?.let { Optional.Present(it) } ?: Optional.Absent,
                arcId = arcId?.let { Optional.Present(it) } ?: Optional.Absent,
                searchTerm = searchTerm?.let { Optional.Present(it) } ?: Optional.Absent
            )
        ).execute()

        response.errors?.firstOrNull()?.let { error ->
            throw Exception(error.message)
        }

        return response.data?.quotes ?: emptyList()
    }

    override suspend fun getQuote(id: Int): GetQuoteQuery.Quote? {
        val response = apolloClient.query(GetQuoteQuery(id)).execute()

        response.errors?.firstOrNull()?.let { error ->
            throw Exception(error.message)
        }

        return (response.data?.quote)
    }
}