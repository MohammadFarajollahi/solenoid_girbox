#include <PWM.h>
int test_motor = 10;
int freq1 = 10;   //10-10000 Hz (I wanted that range only)
int freq2 = 300;  //10-10000 Hz (I wanted that range only)
int duty1 = 0;    //1-65535
int duty2 = 0;    //1-65535
int dutyCycleInput1 = 65535;
int dutyCycleInput2 = 65535;

//********input data******
String data;
String cmnd;
String ss;
String text;

#include <SoftwareSerial.h>
SoftwareSerial mySerial(3, 2); // RX, TX

void setup() {
  Serial.begin(115200);
   mySerial.begin(9600);
  Serial.println("wait...");

  InitTimersSafe();

   SetPinFrequencySafe(test_motor, 300);
}

void loop() {
  motor_control();
  uart_input();
}



