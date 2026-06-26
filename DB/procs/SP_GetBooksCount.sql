DELIMITER $$;
DROP PROCEDURE IF EXISTS `librarydb`.`SP_GetBooksCount`$$
CREATE DEFINER=`root`@`localhost` PROCEDURE `SP_GetBooksCount`(
    IN p_search VARCHAR(255)
)
BEGIN
    SET @s = p_search;

    SET @sql = '
        SELECT COUNT(*) AS Count FROM Books
        WHERE (? = '''' OR Title LIKE CONCAT(''%'',?,''%'') OR Author LIKE CONCAT(''%'',?,''%''))
    ';

    PREPARE stmt FROM @sql;
    EXECUTE stmt USING @s, @s, @s;
    DEALLOCATE PREPARE stmt;
END$$
DELIMITER ;$$