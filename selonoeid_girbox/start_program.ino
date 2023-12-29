
int count_pwm;

void start_program() {



  if (start_prog == 1) {

    if (second < 5 && start_count == 0) {
      start_count = 1;
      Serial.println("state1");
      mySerial.println("md:40");
      dutyCycleInput1 = map(34, 100, 0, 0, 65535);
    }

    if (second >= 10 && second < 20 && start_count == 1) {
      start_count = 2;
      Serial.println("state2");
      dutyCycleInput1 = map(30, 100, 0, 0, 65535);
    }

    if (second >= 20 && second < 30 && start_count == 2) {
      start_count = 3;
      Serial.println("state3");
      dutyCycleInput1 = map(25, 100, 0, 0, 65535);
    }

    if (second >= 30 && second < 40 && start_count == 3) {
      start_count = 4;
      Serial.println("state4");
      dutyCycleInput1 = map(20, 100, 0, 0, 65535);
    }

    if (second >= 40 && second < 50 && start_count == 4) {
      start_count = 5;
      Serial.println("state5");
      dutyCycleInput1 = map(10, 100, 0, 0, 65535);
    }

     if (second >= 50 && second < 60 && start_count == 5) {
      start_count = 6;
      Serial.println("state6");
      dutyCycleInput1 = map(34, 100, 0, 0, 65535);
    }

      if (second >= 60 && second < 70 && start_count == 6) {
      start_count = 7;
      Serial.println("state7");
      dutyCycleInput1 = map(10, 100, 0, 0, 65535);
    }

     if (second >= 70 && second < 80 && start_count == 7) {
      start_count = 8;
      Serial.println("state8");
      dutyCycleInput1 = map(0, 100, 0, 0, 65535);
    }

    if (second >= 80) {
      start_count = 0;
      mySerial.println("md:0");
      delay(500);
      dutyCycleInput1 = map(0, 100, 0, 0, 65535);
      Serial.println("*** end ***");
      start_prog = 0;
      main_timer_count = 0;
      sensor1_send  = 0;
    }

    ////////
  }
}