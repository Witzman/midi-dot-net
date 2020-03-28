// https://github.com/FastLED/FastLED
#include <FastLED.h>
#define NUM_LEDS 60
#define DATA_PIN 5

int redVal = 225;
int greenVal = 70;
int blueVal = 70;
bool allowColorChange = true;

CRGB leds[NUM_LEDS];

void setup() {
  Serial.begin(9600);
  Serial.setTimeout(10);
  FastLED.addLeds<NEOPIXEL, DATA_PIN>(leds, NUM_LEDS);
}

void loop() {
  // serial communication
  while(Serial.available() > 0 ){
    //String midiEvent = Serial.readString();
    int midiEvent = (unsigned char)Serial.read();
    if (midiEvent >= 100) {
      // noteOn event
      controlLeds(midiEvent - 100, true);
    } else {
      // noteOff event
      controlLeds(midiEvent, false);
    }
  }
}

void controlLeds (int note, bool onEvent) {
  if (onEvent) {
    leds[note].red = redVal;
    leds[note].green = greenVal;
    leds[note].blue = blueVal;
    FastLED.show();
  } else {
    leds[note] = CRGB::Black;
    FastLED.show();

    // color effect
    if (allowColorChange) {
      redVal = random(10, 250);
      greenVal = random(10, 250);
      blueVal = random(10, 250);
    }
  }
}
