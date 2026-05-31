DELIMITER $$;

DROP PROCEDURE IF EXISTS `librarydb`.`SP_GetAllBooks`$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `SP_GetAllBooks`()
BEGIN
    SELECT * FROM Books;
END$$

DELIMITER ;$$