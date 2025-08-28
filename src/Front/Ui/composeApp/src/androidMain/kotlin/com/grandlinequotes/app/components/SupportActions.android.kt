package com.grandlinequotes.app.components

import android.app.Activity
import android.content.Context
import android.content.Intent
import android.net.Uri
import android.util.Log
import androidx.compose.runtime.*
import androidx.compose.ui.platform.LocalContext
import com.android.billingclient.api.*

@Composable
actual fun rememberSupportActions(): SupportActions {
    val context = LocalContext.current
    val activity = context as? Activity
    val isReady = remember { mutableStateOf(false) }
    val sku = "support_developer"
    val tag = "SupportActions"

    // Holder para usar el client dentro de callbacks
    val billingClientRef = remember { mutableStateOf<BillingClient?>(null) }

    val purchasesUpdatedListener = remember {
        PurchasesUpdatedListener { br, purchases ->
            Log.d(tag, "onPurchasesUpdated: ${br.responseCode} - ${br.debugMessage}")
            if (br.responseCode == BillingClient.BillingResponseCode.OK && purchases != null) {
                val bc = billingClientRef.value ?: return@PurchasesUpdatedListener
                purchases.forEach { p ->
                    if (p.purchaseState == Purchase.PurchaseState.PURCHASED) {
                        consumeIfNeeded(bc, p)
                    }
                }
            }
        }
    }

    // Cliente de facturación
    val billingClient = remember {
        BillingClient.newBuilder(context)
            .enablePendingPurchases(
                PendingPurchasesParams.newBuilder()
                    .enableOneTimeProducts()
                    .build()
            )
            .setListener(purchasesUpdatedListener)
            .build()
            .also { bc -> billingClientRef.value = bc }
    }

    fun ensureConnection(then: (BillingClient) -> Unit) {
        val bc = billingClientRef.value ?: return
        if (isReady.value) { then(bc); return }
        bc.startConnection(object : BillingClientStateListener {
            override fun onBillingServiceDisconnected() {
                isReady.value = false
                Log.w(tag, "Billing disconnected")
            }
            override fun onBillingSetupFinished(br: BillingResult) {
                Log.d(tag, "Setup finished: ${br.responseCode} - ${br.debugMessage}")
                isReady.value = br.responseCode == BillingClient.BillingResponseCode.OK
                if (isReady.value) then(bc)
            }
        })
    }

    fun openStoreListing(ctx: Context) {
        val id = ctx.packageName
        runCatching {
            ctx.startActivity(Intent(Intent.ACTION_VIEW, Uri.parse("market://details?id=$id")))
        }.onFailure {
            ctx.startActivity(
                Intent(Intent.ACTION_VIEW, Uri.parse("https://play.google.com/store/apps/details?id=$id"))
                    .addFlags(Intent.FLAG_ACTIVITY_NEW_TASK)
            )
        }
    }

    val donate: () -> Unit = donate@{
        // Si no hay Play Store, abre la ficha web y sal
        val hasPlay = runCatching {
            context.packageManager.getPackageInfo("com.android.vending", 0)
        }.isSuccess
        if (!hasPlay) {
            openStoreListing(context)
            return@donate
        }

        ensureConnection { bc ->
            val params = QueryProductDetailsParams.newBuilder()
                .setProductList(
                    listOf(
                        QueryProductDetailsParams.Product.newBuilder()
                            .setProductId(sku)
                            .setProductType(BillingClient.ProductType.INAPP)
                            .build()
                    )
                ).build()

            bc.queryProductDetailsAsync(params) { br, result ->
                Log.d(tag, "queryProductDetails: ${br.responseCode} - ${br.debugMessage}")
                val pd = result.productDetailsList?.firstOrNull()
                if (br.responseCode != BillingClient.BillingResponseCode.OK || pd == null || activity == null) {
                    openStoreListing(context)
                    return@queryProductDetailsAsync
                }

                val flow = BillingFlowParams.newBuilder()
                    .setProductDetailsParamsList(
                        listOf(
                            BillingFlowParams.ProductDetailsParams.newBuilder()
                                .setProductDetails(pd)
                                .build()
                        )
                    ).build()

                bc.launchBillingFlow(activity, flow)
            }
        }
    }

    val rate = { openStoreListing(context) }

    val share = {
        val pkg = context.packageName
        val appName = runCatching {
            context.packageManager.getApplicationLabel(context.applicationInfo).toString()
        }.getOrElse { "this app" }

        // Enlace web (mejor que market:// para compartir)
        val storeUrl = "https://play.google.com/store/apps/details?id=$pkg"

        val text = "Check out $appName on Google Play: $storeUrl"

        val send = Intent(Intent.ACTION_SEND).apply {
            type = "text/plain"
            putExtra(Intent.EXTRA_SUBJECT, appName)   // opcional
            putExtra(Intent.EXTRA_TEXT, text)         // <- incluye el enlace
        }
        val chooser = Intent.createChooser(send, "Share")
        if (context !is Activity) chooser.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK)
        context.startActivity(chooser)
    }

    DisposableEffect(Unit) {
        onDispose { runCatching { billingClient.endConnection() } }
    }

    return SupportActions(donate = donate, rate = rate, share = share)
}

// Consumir la compra (donación)
private fun consumeIfNeeded(bc: BillingClient, purchase: Purchase) {
    val params = ConsumeParams.newBuilder()
        .setPurchaseToken(purchase.purchaseToken)
        .build()
    bc.consumeAsync(params) { _, _ -> /* no-op */ }
}
