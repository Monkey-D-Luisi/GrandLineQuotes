package com.grandlinequotes.app.views

import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.WindowInsets
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.widthIn
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.automirrored.filled.ArrowBack
import androidx.compose.material.icons.outlined.Share
import androidx.compose.material.icons.outlined.StarBorder
import androidx.compose.material3.CenterAlignedTopAppBar
import androidx.compose.material3.ExperimentalMaterial3Api
import androidx.compose.material3.Icon
import androidx.compose.material3.IconButton
import androidx.compose.material3.Scaffold
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import com.grandlinequotes.app.components.SupportHeroCard
import com.grandlinequotes.resources.Localizer
import dev.icerock.moko.resources.compose.stringResource

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun SupportView(
    onBack: () -> Unit,
    onDonate: () -> Unit = {},
    onRate: () -> Unit = {},
    onShare: () -> Unit = {}
) {
    Scaffold(
        contentWindowInsets = WindowInsets(top = 0, bottom = 0),
        topBar = {
            CenterAlignedTopAppBar(
                title = { Text(stringResource(Localizer.strings.support_donate_title)) },
                navigationIcon = {
                    IconButton(onClick = onBack) {
                        Icon(
                            Icons.AutoMirrored.Filled.ArrowBack,
                            contentDescription = stringResource(Localizer.strings.common_cd_back)
                        )
                    }
                }
            )
        }
    ) { padding ->
        Column(
            modifier = Modifier
                .padding(padding)
                .fillMaxSize()
                .padding(horizontal = 24.dp),
            horizontalAlignment = Alignment.CenterHorizontally,
            verticalArrangement = Arrangement.spacedBy(24.dp, Alignment.CenterVertically)
        ) {
            SupportHeroCard(
                onDonate = onDonate,
                headline = stringResource(Localizer.strings.support_thanks_title),
                body = stringResource(Localizer.strings.support_donate_message),
                cta = stringResource(Localizer.strings.support_donate_button),
                modifier = Modifier
                    .fillMaxWidth()
                    .widthIn(max = 520.dp)
            )

            Row(
                modifier = Modifier.widthIn(max = 520.dp),
                horizontalArrangement = Arrangement.spacedBy(12.dp)
            ) {
                androidx.compose.material3.AssistChip(
                    onClick = onRate,
                    label = { Text(stringResource(Localizer.strings.support_rate_action)) },
                    leadingIcon = { Icon(Icons.Outlined.StarBorder, contentDescription = null) }
                )
                androidx.compose.material3.AssistChip(
                    onClick = onShare,
                    label = { Text(stringResource(Localizer.strings.support_share_action)) },
                    leadingIcon = { Icon(Icons.Outlined.Share, contentDescription = null) }
                )
            }
        }
    }
}
