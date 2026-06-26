DELIMITER $$;
DROP PROCEDURE IF EXISTS `librarydb`.`SP_GetAllBooks`$$
CREATE DEFINER=`root`@`localhost` PROCEDURE `SP_GetAllBooks`(
    IN p_search VARCHAR(255),
    IN p_page INT,
    IN p_pageSize INT
)
BEGIN
    SET @s    = p_search;
    SET @lim  = p_pageSize;
    SET @off  = (p_page - 1) * p_pageSize;

    SET @sql = '
        SELECT * FROM Books
        WHERE (? = '''' OR Title LIKE CONCAT(''%'',?,''%'') OR Author LIKE CONCAT(''%'',?,''%''))
        ORDER BY Title
        LIMIT ? OFFSET ?
    ';

    PREPARE stmt FROM @sql;
    EXECUTE stmt USING @s, @s, @s, @lim, @off;
    DEALLOCATE PREPARE stmt;
END$$
DELIMITER ;$$