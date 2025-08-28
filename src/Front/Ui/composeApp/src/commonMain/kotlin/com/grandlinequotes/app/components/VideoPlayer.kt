package com.grandlinequotes.app.components

import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier

@Composable
expect fun VideoPlayer(videoUrl: String, modifier: Modifier = Modifier)
