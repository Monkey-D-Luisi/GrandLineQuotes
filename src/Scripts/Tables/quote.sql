CREATE TABLE `quote` (
        `id` INT(11) NOT NULL AUTO_INCREMENT,
        `original_text` TEXT NULL DEFAULT NULL COLLATE 'utf8mb4_general_ci',
        `text` TEXT NULL DEFAULT NULL COLLATE 'utf8mb4_general_ci',
        `author_id` INT(11) NULL DEFAULT NULL,
        `episode_number` INT(11) NULL DEFAULT NULL,
        `is_reviewed` BIT NOT NULL DEFAULT 0,
        PRIMARY KEY (`id`) USING BTREE,
        INDEX `author` (`author_id`) USING BTREE,
        INDEX `episode` (`episode_number`) USING BTREE,
        CONSTRAINT `author` FOREIGN KEY (`author_id`) REFERENCES `character` (`id`) ON UPDATE CASCADE ON DELETE CASCADE,
        CONSTRAINT `episode` FOREIGN KEY (`episode_number`) REFERENCES `episode` (`number`) ON UPDATE CASCADE ON DELETE CASCADE
)
COLLATE='utf8mb4_general_ci'
ENGINE=InnoDB
;
