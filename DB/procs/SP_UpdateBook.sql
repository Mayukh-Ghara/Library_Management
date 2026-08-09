DELIMITER $$;

DROP PROCEDURE IF EXISTS `librarydb`.`SP_UpdateBook`$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `SP_UpdateBook`(
    IN pId INT,
    IN pTitle VARCHAR(200),
    IN pAuthor VARCHAR(150),
    IN pISBN VARCHAR(20),
    IN pPublishedYear INT,
    IN pCopiesAvailable INT
)
BEGIN
    UPDATE Books
    SET Title = pTitle,
        Author = pAuthor,
        ISBN = pISBN,
        PublishedYear = pPublishedYear,
        CopiesAvailable = pCopiesAvailable
    WHERE ID = pId;
END$$

DELIMITER ;$$