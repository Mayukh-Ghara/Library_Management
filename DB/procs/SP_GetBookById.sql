DELIMITER $$;

DROP PROCEDURE IF EXISTS `librarydb`.`SP_GetBookById`$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `SP_GetBookById`(IN bookId INT)
BEGIN
    SELECT * FROM Books WHERE ID = bookId;
END$$

DELIMITER ;$$