//*****************pwm****************
#include <PWM.h>
int main_motor = 11;
int test_motor = 12;
int freq1 = 10;  //10-10000 Hz (I wanted that range only)
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

//out_in
int led_main = 13;

//timer
int timer_;

//sensor
int sensor1 = A0;
int sensor_read1;
int sensor1_send;
String data_send;

int sensor2 = A1;
int sensor_read2;
int sensor2_send;
int test=0;


#include <SoftwareSerial.h>
SoftwareSerial mySerial(2, 3); // RX, TX

void setup() {
  Serial.begin(115200);
   mySerial.begin(9600);
  Serial.println("wait...");

  pinMode(led_main, OUTPUT);

  //*****************pwm***********
  InitTimersSafe();

  

}
  

void loop() {
  uart_input();
  sensor_read();
  motor_control();
  test_();
}

