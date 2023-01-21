#include <FastLED.h>

#define LED_MATRIX_WIDTH 64
#define LED_MATRIX_HEIGHT 8
#define NUM_MATRIX_UNITS 4
#define NUM_LEDS LED_MATRIX_WIDTH*LED_MATRIX_HEIGHT*NUM_MATRIX_UNITS
#define NUM_VIDEO_BYTES NUM_LEDS*3
//#define PIN 52 //Arduino Mega or Due
#define PIN 5 //ESP32
#define SERIAL_SIZE_RX  32768 //ESP32

#define BRIGHTNESS 0.5

CRGB leds[NUM_LEDS];
char terminator = '0';

byte readBuffer[1];
double bufferLength = 1;

void setup() {
  FastLED.addLeds<NEOPIXEL, PIN>(leds, NUM_LEDS);
  Serial.begin(500000);
  Serial.setRxBufferSize(SERIAL_SIZE_RX); //ESP32
}
  
void loop() {
  // receive data
  if(!Serial.available()){
    leds[0] = CRGB(50,0,0);
    leds[1] = CRGB(0,0,0);
    leds[2] = CRGB(0,0,0);
    FastLED.show();
    //delay(100);
    return;
  }
  byte video_bytes[NUM_VIDEO_BYTES]; //3 bytes per RGB
  if(Serial.readBytesUntil(terminator, video_bytes, NUM_VIDEO_BYTES) == NUM_VIDEO_BYTES){
    for(int u = 0; u < NUM_MATRIX_UNITS; u++) {
      int unit_offset = LED_MATRIX_HEIGHT * LED_MATRIX_WIDTH * u;
      for(int h = 0; h < LED_MATRIX_HEIGHT; h++) {
        for(int w = 0; w < LED_MATRIX_WIDTH; w++) {
          int h_in_loop;
          if(w%2 == 0) {
            h_in_loop = h;
          } else {
            h_in_loop = LED_MATRIX_HEIGHT -1 - h;
          }

          int w_in_loop;
          if(u%2!=0) {
            w_in_loop = w;
          } else {
            w_in_loop = LED_MATRIX_WIDTH -1 -w;
          }
          
          int led_index = unit_offset + LED_MATRIX_HEIGHT * w_in_loop + h_in_loop;
          int data_index = unit_offset + LED_MATRIX_WIDTH * h + w;

          int r = video_bytes[data_index*3];
          int g = video_bytes[data_index*3+1];
          int b = video_bytes[data_index*3+2];
          leds[led_index] = CRGB(r*BRIGHTNESS, g*BRIGHTNESS, b*BRIGHTNESS);
        }
      }
    }
    leds[1] = CRGB(0,50,0);
    leds[2] = CRGB(0,0,0);
  } else {
    leds[1] = CRGB(0,0,0);
    leds[2] = CRGB(0,0,50);
  }
  FastLED.show();
}
