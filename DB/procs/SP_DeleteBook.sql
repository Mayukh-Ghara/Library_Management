DELIMITER $$;

DROP PROCEDURE IF EXISTS `librarydb`.`SP_DeleteBook`$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `SP_DeleteBook`(IN pId INT)
BEGIN
    DELETE FROM Books WHERE ID = pId;
END$$

DELIMITER ;$$