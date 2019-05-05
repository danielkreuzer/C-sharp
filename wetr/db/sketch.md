* station 
    * id: int
    * name: varchar
    * type_id: int          FK- station_type
    * latitude: float
    * longitude: float
    * community_id: int     FK- community
    * creator: int          FK - user

* station_type
    * id: int
    * manufacturer: varchar
    * model: varchar

* community
    * zip_code: int     
    * name: varchar
    * district_id: int  FK - district

* district
    * id: int
    * name: varchar
    * province_id: int  FK - province

* province
    * id: int
    * name: varchar

* user
    * id: int
    * username: varchar
    * password: varchar
    * email: varchar
    * first_name: varchar
    * last_name: varchar
    * date_of_birth: date
    * community_id: int         FK - community

* measurement
    * id: int
    * value: float
    * timestamp: datetime
    * station_id: int           FK - station
    * type_id: int              FK - measurement_type
    * unit_id: int              FK - unit

* measurement_type
    * id: int
    * name: varchar

* unit
    * id: int
    * short_name: varchar
    * long_name: varchar
    