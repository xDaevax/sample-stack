from network import (WLAN, STA_IF, AP_IF)
import network
from shared import (ConfigProvider, Logger)
import machine
import time


class WifiConnection:
    """Type used to manage a wifi connection for a Pico."""

    __config_provider: ConfigProvider
    __logger: Logger
    _connection: WLAN

    def __init__(self, logger: Logger, config_provider: ConfigProvider):
        self.__config_provider = config_provider
        self.__logger = logger
        self._connection = WLAN(STA_IF)

    
    def connect(self):
        """Attempts to connect to a wifi network."""

        network.hostname("Sample Stack Monitor")
        self._connection.active(True)
        self._connection.scan()

        if (self._connection.isconnected()):
            self.__logger.info("Already connected.")
            return

        wifi_user = self.__config_provider.read_wifi_user()
        wifi_pass = self.__config_provider.read_wifi_password()
        wifi_ssid = self.__config_provider.read_wifi_ssid()
        wifi_key = self.__config_provider.read_wifi_key()

        self._connection.connect(wifi_ssid, wifi_key)

        while not self._connection.isconnected():
            machine.idle()
            time.sleep_ms(100)
        
        self.__logger.info("Connected to: %s" %(wifi_ssid))

    def connection_info(self):
        """Returns connection info."""
        return self._connection.ipconfig('addr4')

    def disconnect(self):
        """Disconnects from the wifi network."""
        self._connection.disconnect()
