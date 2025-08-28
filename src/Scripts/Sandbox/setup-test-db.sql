-- Test DB setup for grandlinequotes
-- Contains schema and seed data

DROP DATABASE IF EXISTS `grandlinequotes`;
CREATE DATABASE `grandlinequotes` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci */;
USE `grandlinequotes`;

-- base tables with no dependencies
CREATE TABLE IF NOT EXISTS `filler_type` (
  `id` tinyint(4) NOT NULL AUTO_INCREMENT,
  `value` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
INSERT IGNORE INTO `filler_type` (`id`, `value`) VALUES
  (1, 'none'),
  (2, 'partial'),
  (3, 'full');

CREATE TABLE IF NOT EXISTS `saga` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
INSERT IGNORE INTO `saga` (`id`) VALUES
  (1),
  (5);

-- dependent tables
CREATE TABLE IF NOT EXISTS `arc` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `saga_id` int(11) DEFAULT NULL,
  `filler_type_id` tinyint(4) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `FK_arc_saga` (`saga_id`) USING BTREE,
  KEY `FK_arc_filler_type` (`filler_type_id`),
  CONSTRAINT `FK_arc_filler_type` FOREIGN KEY (`filler_type_id`) REFERENCES `filler_type` (`id`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `FK_arc_saga` FOREIGN KEY (`saga_id`) REFERENCES `saga` (`id`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
INSERT IGNORE INTO `arc` (`id`, `saga_id`, `filler_type_id`) VALUES
  (1, 1, 1),
  (5, 5, 1);

CREATE TABLE IF NOT EXISTS `arc_title` (
  `arc_id` int(11) NOT NULL,
  `language_code` varchar(2) NOT NULL,
  `value` varchar(150) DEFAULT NULL,
  PRIMARY KEY (`arc_id`,`language_code`),
  CONSTRAINT `FK_arc_title_arc` FOREIGN KEY (`arc_id`) REFERENCES `arc` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
INSERT IGNORE INTO `arc_title` (`arc_id`, `language_code`, `value`) VALUES
  (1, 'en', 'Impel Down'),
  (1, 'es', 'Impel Down'),
  (5, 'en', 'Dressrosa'),
  (5, 'es', 'Dressrosa');

CREATE TABLE IF NOT EXISTS `character` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(50) DEFAULT NULL,
  `alias` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=290 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
INSERT IGNORE INTO `character` (`id`, `name`, `alias`) VALUES
  (1, 'Crocodile', 'Mr. 0'),
  (210, 'Fujitora', 'Issho');

CREATE TABLE IF NOT EXISTS `episode` (
  `number` int(11) NOT NULL,
  `arc_id` int(11) DEFAULT NULL,
  PRIMARY KEY (`number`),
  KEY `FK_episode_arc` (`arc_id`) USING BTREE,
  CONSTRAINT `FK_episode_arc` FOREIGN KEY (`arc_id`) REFERENCES `arc` (`id`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
INSERT IGNORE INTO `episode` (`number`, `arc_id`) VALUES
  (442, 1),
  (736, 5);

CREATE TABLE IF NOT EXISTS `episode_title` (
  `episode_number` int(11) NOT NULL,
  `language_code` varchar(2) NOT NULL,
  `value` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`episode_number`,`language_code`),
  CONSTRAINT `FK_episode_title_episode` FOREIGN KEY (`episode_number`) REFERENCES `episode` (`number`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
INSERT IGNORE INTO `episode_title` (`episode_number`, `language_code`, `value`) VALUES
  (442, 'en', 'Ace\'s Convoy Starts - The Offense and Defense of the Lowest Level, Level 6!'),
  (442, 'es', 'El traslado bajo custodia de Ace - ¡Ofensiva y defensiva del nivel 6!'),
  (736, 'en', 'Sending a Shock Wave - The Worst Generation Goes Into Action!'),
  (736, 'es', 'El mundo tiembla. ¡La Peor Generación sigue avanzando!');

CREATE TABLE IF NOT EXISTS `quote` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `original_text` text DEFAULT NULL,
  `text` text DEFAULT NULL,
  `author_id` int(11) DEFAULT NULL,
  `episode_number` int(11) DEFAULT NULL,
  `is_reviewed` bit NOT NULL DEFAULT 0,
  PRIMARY KEY (`id`),
  KEY `FK_quote_episode` (`episode_number`),
  KEY `FK_quote_character` (`author_id`) USING BTREE,
  CONSTRAINT `FK_quote_character` FOREIGN KEY (`author_id`) REFERENCES `character` (`id`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `FK_quote_episode` FOREIGN KEY (`episode_number`) REFERENCES `episode` (`number`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=521 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
INSERT IGNORE INTO `quote` (`id`, `original_text`, `text`, `author_id`, `episode_number`, `is_reviewed`) VALUES
  (1, '久しぶりだな、麦わら', 'Hisashiburi dana Mugiwara!', 1, 442, 1),
  (411, '不備を認めたくらいで地に落ちる信頼など、もともとねぇも同じだ。', 'Fubi o mitometa kurai de chi ni ochiru shinrai nado, motomoto nē mo onaji da.', 210, 736, 1);

CREATE TABLE IF NOT EXISTS `quote_translation` (
  `quote_id` int(11) NOT NULL,
  `language_code` varchar(2) NOT NULL,
  `value` text DEFAULT NULL,
  PRIMARY KEY (`quote_id`,`language_code`),
  CONSTRAINT `FK_quote_translation_quote` FOREIGN KEY (`quote_id`) REFERENCES `quote` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
INSERT IGNORE INTO `quote_translation` (`quote_id`, `language_code`, `value`) VALUES
  (1, 'en', 'It\'s been a while, Straw Hat'),
  (1, 'es', 'Cuánto tiempo, Sombrero de Paja'),
  (411, 'en', 'If trust can be destroyed just by admitting your mistakes, then it wasn’t real trust to begin with'),
  (411, 'es', 'Si la confianza se rompe solo por reconocer tus errores, entonces nunca fue verdadera confianza');

CREATE TABLE IF NOT EXISTS `saga_title` (
  `saga_id` int(11) NOT NULL,
  `language_code` varchar(2) NOT NULL,
  `value` varchar(150) DEFAULT NULL,
  PRIMARY KEY (`saga_id`,`language_code`),
  CONSTRAINT `FK_saga_title_saga` FOREIGN KEY (`saga_id`) REFERENCES `saga` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
INSERT IGNORE INTO `saga_title` (`saga_id`, `language_code`, `value`) VALUES
  (1, 'en', 'Summit War'),
  (1, 'es', 'Cumbre de la guerra'),
  (5, 'en', 'Dressrosa'),
  (5, 'es', 'Dressrosa');
