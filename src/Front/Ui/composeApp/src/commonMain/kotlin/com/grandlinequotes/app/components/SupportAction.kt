package com.grandlinequotes.app.components

import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.outlined.VolunteerActivism
import androidx.compose.material3.ExperimentalMaterial3Api
import androidx.compose.material3.Icon
import androidx.compose.material3.IconButton
import androidx.compose.material3.Text
import androidx.compose.material3.TooltipBox
import androidx.compose.material3.TooltipDefaults
import androidx.compose.material3.rememberTooltipState
import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import com.grandlinequotes.resources.Localizer
import dev.icerock.moko.resources.compose.stringResource

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun SupportAction(onClick: () -> Unit) {
    val state = rememberTooltipState()
    TooltipBox(
        positionProvider = TooltipDefaults.rememberPlainTooltipPositionProvider(),
        tooltip = { Text(stringResource(Localizer.strings.support_donate_action)) },
        state = state
    ) {
        IconButton(onClick = onClick) {
            Icon(
                imageVector = Icons.Outlined.VolunteerActivism,
                contentDescription = stringResource(Localizer.strings.support_donate_action)
            )
        }
    }
}
