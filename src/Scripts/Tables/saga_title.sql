CREATE TABLE `saga_title` (
        `saga_id` INT(11) NOT NULL,
        `language_code` VARCHAR(2) NOT NULL COLLATE 'utf8mb4_general_ci',
        `value` VARCHAR(150) NULL DEFAULT NULL COLLATE 'utf8mb4_general_ci',
        PRIMARY KEY (`saga_id`, `language_code`) USING BTREE,
        CONSTRAINT `saga_title_saga` FOREIGN KEY (`saga_id`) REFERENCES `saga` (`id`) ON UPDATE CASCADE ON DELETE CASCADE
)
COLLATE='utf8mb4_general_ci'
ENGINE=InnoDB
;
