unsigned long elapsedTime, previousTime;
//*****************pwm****************
#include <PWM.h>
int main_motor = 11;
int test_motor = 12;
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

//out_in
int led_main = 13;

//timer
int timer_count;

//sensor
int sensor1 = A0;

int sensor1_send;
String data_send;

int sensor2 = A1;

int sensor2_send;
int test = 0;

int start_prog;

float second;
int start_count;

#include <SoftwareSerial.h>
SoftwareSerial mySerial(2, 3);  // RX, TX
int main_timer_count;
int main_timer;

//***************************main timer*********************
byte value = 98;
volatile boolean flagComplete = false;

void setup() {
  Serial.begin(115200);
  mySerial.begin(9600);
  Serial.println("wait...");

  pinMode(led_main, OUTPUT);

  //*****************pwm***********
  InitTimersSafe();

  //*******timer*********
  noInterrupts();  // disable all interrupts
  TCCR2A = 0;
  TCCR2B = 0;
  TCNT2 = value;                                      // preload timer
  TCCR2B |= (1 << CS22) | (1 << CS21) | (1 << CS20);  // 1024 prescaler
  TIMSK2 |= (1 << TOIE2);                             // enable timer overflow interrupt ISR
  interrupts();

  SetPinFrequencySafe(main_motor, 100);
}



void loop() {
  uart_input();
  sensor_read();
  motor_control();
  test_();
  start_program();
  //Serial.println(second);
}


ISR(TIMER2_OVF_vect)  // interrupt service routine for overflow
{
  static byte count = 0;
  count++;
  if (count == 100) {
    count = 0;
    if (start_prog) ++second;
  }
  TCNT2 = value;  // preload timer
}