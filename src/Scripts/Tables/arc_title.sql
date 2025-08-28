CREATE TABLE `arc_title` (
        `arc_id` INT(11) NOT NULL,
        `language_code` VARCHAR(2) NOT NULL COLLATE 'utf8mb4_general_ci',
        `value` VARCHAR(150) NULL DEFAULT NULL COLLATE 'utf8mb4_general_ci',
        PRIMARY KEY (`arc_id`, `language_code`) USING BTREE,
        CONSTRAINT `arc_title_arc` FOREIGN KEY (`arc_id`) REFERENCES `arc` (`id`) ON UPDATE CASCADE ON DELETE CASCADE
)
COLLATE='utf8mb4_general_ci'
ENGINE=InnoDB
;
