from io import (open)
from shared import (Logger)


class FileIOProvider:
    """Class used to interact with the Pico file system."""
    
    __logger: Logger

    def __init__(self, logger: Logger):
        """Initializes a new instance of the FileProvider class.
        :param logger: The logger instance used to write log messages."""
        self.__logger = logger

    def write_file(self, path: str, data: str):
        """Writes the given data to a file with the specified name.
        :param path: The path to the file being written (must be absolute).
        :param data: The data to write to the file."""
        self.__logger.info("Attempting to write data to file: '%s'" % (str(path)))
        with open(path, 'w') as file:
            file.write(data)

    def read_file_as_string(self, path: str) -> str:
        """Reads the data of the specified file as a string.
        :param path: The absolute path to a file being read."""
        self.__logger.info("Attempting to read data from file: '%s'" % (str(path)))
        contents = ""

        with open(path) as file:
            contents = file.read()
        
        return contents
