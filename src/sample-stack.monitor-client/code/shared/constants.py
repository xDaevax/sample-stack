MAX_INT: int = 65535
"""The maximum value for an integer (to simplify other constants that are specific in function but share the same value)."""
MAX_ADC: int = MAX_INT
"""The maximum voltage for analog-to-digital signals (10,000 mV in decimal notation)."""
PICO_ONBOARD_LED_PIN: int = 25
"""The pin number for the on-board LED for a Raspberry PI Pico (note that the Pico W uses a named pin called "LED")"""
PICO_W_ONBOARD_LED_PIN: str = "LED"
"""The pin number for the on-board LED for a Raspberry PI Pico (note that the Pico W uses a named pin called "LED")"""
CONFIG_FILE_PATH: str = "settings.ini"
"""The full path to the settings file used to configure behavior for the Pico."""
