package com.grandlinequotes.app

import android.content.Context
import com.google.android.play.core.integrity.IntegrityManager
import com.google.android.play.core.integrity.IntegrityManagerFactory
import com.google.android.play.core.integrity.IntegrityServiceException
import com.google.android.play.core.integrity.IntegrityTokenRequest
import com.grandlinequotes.Config
import com.grandlinequotes.network.CustomHttpClient
import com.grandlinequotes.api.IntegritySession
import okhttp3.MediaType.Companion.toMediaType
import okhttp3.OkHttpClient
import okhttp3.Request
import okhttp3.RequestBody.Companion.toRequestBody
import org.json.JSONObject
import java.util.UUID
import java.util.concurrent.Executors

class PlayIntegrity(
    private val context: Context,
    private val http: OkHttpClient = CustomHttpClient(addAuth = false)
) {
    private val integrityManager: IntegrityManager = IntegrityManagerFactory.create(context)
    private val bg = Executors.newSingleThreadExecutor()

    fun fetchSessionToken() {
        val nonce = UUID.randomUUID().toString()

        val request = IntegrityTokenRequest.builder()
            .setNonce(nonce)
            .build()

        integrityManager.requestIntegrityToken(request)
            .addOnSuccessListener { response ->
                val token = response.token()
                exchangeForSession(token, nonce)  // ← sin auth
            }
            .addOnFailureListener { e ->
                if (e is IntegrityServiceException) e.printStackTrace()
            }
    }

    private fun exchangeForSession(token: String, nonce: String) {
        bg.execute {
            try {
                val url = "${Config.apiBaseUrl.trimEnd('/')}/integrity/exchange"

                val req = Request.Builder()
                    .url(url)
                    .post("".toRequestBody("application/json".toMediaType()))
                    .addHeader("X-Play-Integrity-Token", token)
                    .addHeader("X-Play-Integrity-Nonce", nonce)
                    .addHeader("X-Play-Integrity-Package-Name", context.packageName)
                    .build()

                http.newCall(req).execute().use { resp ->
                    val body = resp.body?.string().orEmpty()
                    val session = JSONObject(body).optString("session")
                    IntegritySession.token = session
                }
            } catch (t: Throwable) {
                t.printStackTrace()
            }
        }
    }
}
