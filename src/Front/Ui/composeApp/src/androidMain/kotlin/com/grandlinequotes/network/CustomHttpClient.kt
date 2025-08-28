package com.grandlinequotes.network

import okhttp3.Dns
import okhttp3.OkHttpClient
import okhttp3.dnsoverhttps.DnsOverHttps
import okhttp3.HttpUrl.Companion.toHttpUrl
import java.io.IOException
import java.net.InetAddress
import java.net.UnknownHostException

fun CustomHttpClient(
    provider: TokenProvider = IntegrityTokenProvider,
    addAuth: Boolean
): OkHttpClient {
    val bootstrap = OkHttpClient.Builder()
        .retryOnConnectionFailure(true)
        .build()

    // Cloudflare DoH
    val cfDoH = DnsOverHttps.Builder()
        .client(bootstrap)
        .url("https://cloudflare-dns.com/dns-query".toHttpUrl())
        .bootstrapDnsHosts(
            InetAddress.getByName("1.1.1.1"),
            InetAddress.getByName("1.0.0.1"),
            InetAddress.getByName("2606:4700:4700::1111"),
            InetAddress.getByName("2606:4700:4700::1001")
        )
        .includeIPv6(true)
        .build()

    // Google DoH (respaldo)
    val googleDoH = DnsOverHttps.Builder()
        .client(bootstrap)
        .url("https://dns.google/dns-query".toHttpUrl())
        .bootstrapDnsHosts(
            InetAddress.getByName("8.8.8.8"),
            InetAddress.getByName("8.8.4.4"),
            InetAddress.getByName("2001:4860:4860::8888"),
            InetAddress.getByName("2001:4860:4860::8844")
        )
        .includeIPv6(true)
        .build()

    // Cadena de DNS: CF → Google → Sistema
    val chainDns = object : Dns {
        override fun lookup(hostname: String): List<InetAddress> {
            var last: IOException? = null
            for (resolver in listOf<Dns>(cfDoH, googleDoH, Dns.SYSTEM)) {
                try {
                    val res = resolver.lookup(hostname)
                    if (res.isNotEmpty()) return res
                } catch (e: IOException) {
                    last = e
                }
            }
            throw last ?: UnknownHostException(hostname)
        }
    }

    val builder = OkHttpClient.Builder()
        .dns(chainDns)
        .retryOnConnectionFailure(true)
        // Opcional: ajusta timeouts; para vídeo puedes subir readTimeout
        //.connectTimeout(10, TimeUnit.SECONDS)
        //.readTimeout(30, TimeUnit.SECONDS)
        //.writeTimeout(30, TimeUnit.SECONDS)
        .apply {
            if (addAuth) addInterceptor(OkHttpAuthInterceptor(provider))
        }

    return builder.build()
}

