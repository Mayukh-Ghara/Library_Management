DELIMITER $$;

DROP PROCEDURE IF EXISTS `librarydb`.`SP_AddBook`$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `SP_AddBook`(
    IN pTitle VARCHAR(200),
    IN pAuthor VARCHAR(150),
    IN pISBN VARCHAR(20),
    IN pPublishedYear INT,
    IN pCopiesAvailable INT
)
BEGIN
    INSERT INTO Books (Title, Author, ISBN, PublishedYear, CopiesAvailable)
    VALUES (pTitle, pAuthor, pISBN, pPublishedYear, pCopiesAvailable);
END$$

DELIMITER ;$$