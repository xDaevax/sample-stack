from machine import (Pin)
import time
from shared import (Logger)

class InputMonitor:
    """Class used to monitor input (often a button)."""

    _button: Pin
    __logger: Logger
    _interrupt: bool
    _debounce_timer: int

    def __init__(self, logger: Logger, gpio_num: int, interrupt: bool):
        """Creates a new instance of the InputMonitor class.
        :param logger: The Logger instance used to write logs.
        :param gpio_num: The GPIO number where the input device is configured.
        :param interrupt: A boolean value used to interrupt the infinite button loop (default to false to enable listening.)"""
        self._button = Pin(gpio_num, Pin.IN, Pin.PULL_DOWN)
        self._interrupt = interrupt
        self._debounce_timer = time.ticks_ms()
        self.__logger = logger
    
    def listen(self, handler):
        """Use this method to provide a callback that is executed when the button is pressed.
        :param handler: A function to execute when the button is pressed."""
        while self._interrupt is False:
            if self._button.value() == 1:
                current_time = time.ticks_ms()

                time_passed = time.ticks_diff(current_time, self._debounce_timer)

                if time_passed > 500:
                    self._debounce_timer = time.ticks_ms()
                    self.__logger.debug("Button press")
                    handler()
                else:
                    self.__logger.debug("Debounce Detection")
        self.__logger.debug("Hanging up the phone")