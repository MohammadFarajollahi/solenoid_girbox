float pressure_sensor1;
float pressure_sensor2;
//***********sensor_read***********
void sensor_read() {
  sensor_read1 = analogRead(sensor1);

  sensor_read2 = analogRead(sensor2);

  if (sensor1_send == 1) {

    pressure_sensor1 = sensor_read1 / 50;  //36
    pressure_sensor2 = sensor_read2 / 36.75;
    if (pressure_sensor1 < 0) pressure_sensor1 = 0;
    if (pressure_sensor2 < 0) pressure_sensor2 = 0;
    
    //data_send = "sensor1:" + String(pressure_sensor1) + ":" + "37.5" + ":" + "12v" + "B"  + "sensor2: " + String(pressure_sensor1) + ":" + "37.5" + ":" + "12v" + "B";
    data_send = "sensor1:" + String(pressure_sensor1) + ":" + "37.5" + ":" + "12v" + "B" + "sensor2:" + String(pressure_sensor2) + ":" + "38.5" + ":" + "15v" + "B / C";
    //data_send = "sensor1:" + String(random(0, 300)) + "B";
    Serial.println(data_send);
    delay(500);

    //Serial.println("s1,34,129,87Bs2,24,19,97Bs3,24,19,97Bs4,24,19,97B");
  }
}
