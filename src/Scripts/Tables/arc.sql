CREATE TABLE `arc` (
        `id` INT(11) NOT NULL AUTO_INCREMENT,
        `filler_type_id` INT(11) NULL DEFAULT NULL,
        `saga_id` INT(11) NULL DEFAULT NULL,
        PRIMARY KEY (`id`) USING BTREE,
        INDEX `saga` (`saga_id`) USING BTREE,
        CONSTRAINT `saga` FOREIGN KEY (`saga_id`) REFERENCES `saga` (`id`) ON UPDATE CASCADE ON DELETE CASCADE
)
COLLATE='utf8mb4_general_ci'
ENGINE=InnoDB
;
