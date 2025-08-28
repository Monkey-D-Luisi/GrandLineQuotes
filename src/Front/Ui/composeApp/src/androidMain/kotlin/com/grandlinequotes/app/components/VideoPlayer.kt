package com.grandlinequotes.app.components

import android.content.Context
import androidx.annotation.OptIn
import androidx.compose.foundation.background
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Fullscreen
import androidx.compose.material.icons.filled.FullscreenExit
import androidx.compose.material3.Icon
import androidx.compose.material3.IconButton
import androidx.compose.runtime.Composable
import androidx.compose.runtime.DisposableEffect
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.viewinterop.AndroidView
import androidx.compose.ui.window.Dialog
import androidx.compose.ui.window.DialogProperties
import androidx.compose.ui.unit.dp
import androidx.media3.common.MediaItem
import androidx.media3.datasource.okhttp.OkHttpDataSource
import androidx.media3.exoplayer.ExoPlayer
import androidx.media3.exoplayer.source.DefaultMediaSourceFactory
import androidx.media3.ui.AspectRatioFrameLayout
import androidx.media3.ui.PlayerView
import com.grandlinequotes.network.CustomHttpClient
import androidx.media3.common.util.UnstableApi

@OptIn(UnstableApi::class)
@Composable
actual fun VideoPlayer(videoUrl: String, modifier: Modifier) {
    val context = LocalContext.current
    var fullScreen by remember { mutableStateOf(false) }

    val okHttp = remember { CustomHttpClient(addAuth = true) }
    val dsFactory = remember {
        OkHttpDataSource.Factory(okHttp)
    }

    // Crea el player UNA vez y cambia el MediaItem al cambiar la URL
    val exoPlayer = remember {
        ExoPlayer.Builder(context)
            .setMediaSourceFactory(
                DefaultMediaSourceFactory(dsFactory)
            )
            .build()
    }

    // Cambia el item cuando cambia la URL
    LaunchedEffect(videoUrl) {
        val item = MediaItem.fromUri(videoUrl)
        exoPlayer.setMediaItem(item)
        exoPlayer.prepare()
        exoPlayer.playWhenReady = true
    }

    DisposableEffect(exoPlayer) { onDispose { exoPlayer.release() } }

    val makeView: (Context) -> PlayerView = { ctx ->
        PlayerView(ctx).apply {
            player = exoPlayer
            useController = true
            setResizeMode(AspectRatioFrameLayout.RESIZE_MODE_FIT)
            setShowBuffering(PlayerView.SHOW_BUFFERING_WHEN_PLAYING)
        }
    }

    if (fullScreen) {
        Dialog(
            onDismissRequest = { fullScreen = false },
            properties = DialogProperties(usePlatformDefaultWidth = false)
        ) {
            Box(Modifier.fillMaxSize()) {
                AndroidView(factory = makeView, modifier = Modifier.fillMaxSize())
                IconButton(
                    onClick = { fullScreen = false },
                    modifier = Modifier
                        .align(Alignment.BottomEnd)
                        .padding(8.dp)
                        .background(Color.Black.copy(alpha = 0.6f), CircleShape)
                ) {
                    Icon(Icons.Filled.FullscreenExit, contentDescription = "Exit fullscreen", tint = Color.White)
                }
            }
        }
    }

    Box(modifier) {
        AndroidView(factory = makeView, modifier = Modifier.fillMaxSize())
        IconButton(
            onClick = { fullScreen = true },
            modifier = Modifier
                .align(Alignment.BottomEnd)
                .padding(8.dp)
                .background(Color.Black.copy(alpha = 0.6f), CircleShape)
        ) {
            Icon(Icons.Filled.Fullscreen, contentDescription = "Fullscreen", tint = Color.White)
        }
    }
}

