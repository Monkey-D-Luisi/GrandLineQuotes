CREATE TABLE `quote_translation` (
        `quote_id` INT(11) NOT NULL,
        `language_code` VARCHAR(2) NOT NULL COLLATE 'utf8mb4_general_ci',
        `value` TEXT NULL DEFAULT NULL COLLATE 'utf8mb4_general_ci',
        PRIMARY KEY (`quote_id`, `language_code`) USING BTREE,
        CONSTRAINT `quote_translation_quote` FOREIGN KEY (`quote_id`) REFERENCES `quote` (`id`) ON UPDATE CASCADE ON DELETE CASCADE
)
COLLATE='utf8mb4_general_ci'
ENGINE=InnoDB
;
