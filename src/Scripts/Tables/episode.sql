CREATE TABLE `episode` (
        `number` INT(11) NOT NULL,
        `arc_id` INT(11) NULL DEFAULT NULL,
        PRIMARY KEY (`number`) USING BTREE,
        INDEX `arc` (`arc_id`) USING BTREE,
        CONSTRAINT `arc` FOREIGN KEY (`arc_id`) REFERENCES `arc` (`id`) ON UPDATE CASCADE ON DELETE CASCADE
)
COLLATE='utf8mb4_general_ci'
ENGINE=InnoDB
;
