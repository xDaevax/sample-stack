import os
import errno
from shared import (Logger, FileIOProvider, constants)


class ConfigProvider:
    """Class used to wrap calls to load configuration data."""

    __logger: Logger
    __file_provider: FileIOProvider
    _settings: dict[str, str]

    def __init__(self, logger: Logger, io_provider: FileIOProvider):
        """Initializes a new instance of the ConfigProvider class.
        :param logger: The logger instance used to write log messages.
        :param io_provider: The FileIOProvider used to read configuration data from the file-system."""
        self.__logger = logger
        self.__file_provider = io_provider
        self._settings = {}

    def _load_file_contents(self) -> str:
        """Attempts to load the raw setting contents from the source config file."""
        contents = self.__file_provider.read_file_as_string(constants.CONFIG_FILE_PATH)

        print(contents)

        if contents is None or len(contents) <= 0:
            raise FileNotFoundError(errno.ENOENT, os.strerror(errno.ENOENT), constants.CONFIG_FILE_PATH)

        return contents

    def _load_from_file(self):
        """Loads the configuration file raw contents"""

        if len(self._settings) > 0:
            self.__logger.info("Settings already loaded.")
            return

        contents = self._load_file_contents()

        split_contents:list[str] = contents.split('\n')

        for item in split_contents:
            if item.startswith('#'):
                continue

            split_setting: list[str] = item.split('=')
            self._settings[split_setting[0].strip()] = split_setting[1].strip().strip('"')

    def read(self, key: str) -> str | None:
        """Attempts to read a value from configuration (after checking for its existence) and returns None if the key is not found.
        :param key: The key to read."""

        if self.has_key(key):
            return self._settings[key]

        self.__logger.warn("The given key " + str(key) + ", could not be found in configuration.")
        return None

    def has_key(self, key: str) -> bool:
        """Determines whether or not the given key is defined in configuration.
        :param key: The name of the configuration item to check."""
        self._load_from_file()
        return key in self._settings

    def read_wifi_password(self) -> str | None:
        """Reads the wifi password."""
        return self.read('wifi_pass')

    def read_wifi_user(self) -> str | None:
        """Reads the wifi username."""
        return self.read('wifi_user')

    def read_wifi_key(self) -> str | None:
        """Reads the wifi security key."""
        return self.read('wifi_key')

    def read_wifi_ssid(self) -> str | None:
        """Reads the wifi ssid"""
        return self.read('wifi_ssid')
