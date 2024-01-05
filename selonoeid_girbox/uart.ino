

///**********************input_uart**********************
void uart_input() {
  while (Serial.available() > 0) {
    //cmnd = Serial.readString();
    cmnd = Serial.readStringUntil('\n');
    //lcd_print();
    uart_text();
  }
}

void uart_text() {
  //Serial.println(cmnd);

  //test///
  if (cmnd == "test on") test = 1;
  if (cmnd == "test off") test = 0;


  //*****manual move******
  data = cmnd.substring(0, 2);
  if (data == "hi") Serial.println("Hello Hohammad");

//***********************************start program***************************
  data = cmnd.substring(0, 13);
  if (data == "start_program") {
    start_prog = 1;
    Serial.println("start program on");
    second = 0;
    start_count = 0;
    main_timer_count = 1;
    sensor1_send =1;
  }
  data = cmnd.substring(0, 12);
  if (data == "stop_program") {
    start_prog = 0;
    Serial.println("start program off");
    second = 0;
    start_count = 0;
    mySerial.println("md=0");
    dutyCycleInput1 = map(0, 100, 0, 0, 65535);
    main_timer_count = 0;
    sensor1_send = 0;
  }  


  data = cmnd.substring(0, 2);
  if (data == "m1") {
    ss = cmnd.substring(2, 10);
    Serial.print("Motor on - count:");
    Serial.println(ss);
  }


  if (data == "p1") {
    Serial.println("sensor1 on");
    sensor1_send = 1;
  }

  if (data == "p2") {
    Serial.println("sensor1 off");
    sensor1_send = 0;
  }


  //*********************motor1********************
  if (data == "td") {  //main motor duty
    String ss;
    ss = cmnd.substring(3, 10);
    duty1 = ss.toFloat();
    ss = "main motor duty:" + String(duty1) + "%";
    Serial.println(ss);
    dutyCycleInput1 = map(duty1, 100, 0, 0, 65535);
  }


  if (data == "tf") {  //main motor frequency
    String ss;
    ss = cmnd.substring(3, 10);
    freq1 = ss.toFloat();
    ss = "main motor frequency:" + String(freq1) + "Hz";
    Serial.println(ss);
    SetPinFrequencySafe(main_motor, freq1);
  }


  //*********************motor2********************
  if (data == "md") {  //test motor duty
    mySerial.println(cmnd);
    // String ss;
    // ss = cmnd.substring(3, 10);
    // duty2 = ss.toFloat();
    // ss = "test motor duty:" + String(duty2) + "%";
    // Serial.println(ss);
    // dutyCycleInput2 = map( duty2 , 100 ,0 , 0 , 65535);
  }


  if (data == "mf") {  //test motor frequency
    mySerial.println(cmnd);
    // String ss;
    // ss = cmnd.substring(3, 10);
    // freq2 = ss.toFloat();
    // ss = "test motor frequency:" + String(freq2) + "Hz";
    // Serial.println(ss);
    // SetPinFrequencySafe(test_motor, freq2);
  }
}
