CREATE TABLE `episode_title` (
        `episode_number` INT(11) NOT NULL,
        `language_code` VARCHAR(2) NOT NULL COLLATE 'utf8mb4_general_ci',
        `value` VARCHAR(150) NULL DEFAULT NULL COLLATE 'utf8mb4_general_ci',
        PRIMARY KEY (`episode_number`, `language_code`) USING BTREE,
        CONSTRAINT `episode_title_episode` FOREIGN KEY (`episode_number`) REFERENCES `episode` (`number`) ON UPDATE CASCADE ON DELETE CASCADE
)
COLLATE='utf8mb4_general_ci'
ENGINE=InnoDB
;
