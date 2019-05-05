$schema=@'
create schema if not exists WetrDB;
ALTER DATABASE WetrDB CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
use WetrDB;

drop table if exists measurement;
drop table if exists station;
drop table if exists station_type;
drop table if exists user;
drop table if exists community;
drop table if exists district;
drop table if exists province;
drop table if exists measurement_type;
drop table if exists unit;

create table province
(
  id   int auto_increment
    primary key,
  name varchar(100) not null
);

create table district
(
  id          int auto_increment
    primary key,
  name        varchar(100) not null,
  province_id int          null,
  constraint district_province_id_fk
  foreign key (province_id) references province (id)
);

create table community
(
    id          int auto_increment
    primary key,
  zip_code    int          not null,
  name        varchar(100) not null,
  district_id int          not null,
  constraint community_district_id_fk
  foreign key (district_id) references district (id)
);

create table station_type
(
  id             int auto_increment
    primary key,
  manufacturer   varchar(100)           null,
  model          varchar(100)           null
);

create table user
(
  id            int auto_increment
    primary key,
  username      varchar(50)  not null,
  password      varchar(100) not null,
  email         varchar(50)  null,
  first_name    varchar(50)  null,
  last_name     varchar(50)  null,
  date_of_birth date    null,
  community_id  int          null,
  constraint user_community_id_fk
  foreign key (community_id) references community (id),
  constraint user_username_uindex
  unique (username)
);

create table station
(
  id           int auto_increment
    primary key,
  name         varchar(100) not null,
  type_id      int          null,
  latitude     float        not null,
  longitude    float        not null,
  community_id int          null,
  altitude     float        null,
  creator      int          not null,
  constraint station_community_zip_code_fk
  foreign key (community_id) references community (id),
  constraint station_station_type_id_fk
  foreign key (type_id) references station_type (id),
  constraint station_user_id_fk
  foreign key (creator) references user(id)
);

create table unit
(
  id   int auto_increment
    primary key,
  short_name varchar(10) not null,
  long_name varchar(30) null
);

create table measurement_type
(
  id           int auto_increment
    primary key,
  name          varchar(30) not null
);

create table measurement
(
  id                int auto_increment
    primary key,
  value             float       null,
  timestamp         datetime    null,
  station_id        int         not null,
  type_id           int         not null,
  unit_id     int not null,
  constraint type_unit_id_fk
  foreign key (unit_id) references unit (id),
  constraint measurement_station_id_fk
  foreign key (station_id) references station (id),
  constraint measurement_type_id_fk
  foreign key (type_id) references measurement_type (id)
);
'@

echo $schema | docker exec -i mysql mysql
