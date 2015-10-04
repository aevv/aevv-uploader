<?php
  require_once("../../private/data-access/pgsql.php");
  require_once("../../private/secrets.php");

  $_pgConnection = new PGDataAccess(Secrets::$Host, Secrets::$Username, Secrets::$Password, Secrets::$Database);
  $_pgConnection->Connect();

  $userName = GetFromPost("username");
  $password = GetFromPost("password");

  Process($_pgConnection, $userName, $password);

  // TODO: de-dupe all duplicated code between here and register.php

  function Process($_connection, $userName, $password) {
    $id = CheckUserExists($_connection, $userName, $password);
    // TODO: permanent storage of keys - save key vs upload, ip vs key, etc.
    ClearOldKey($_connection, $id);
    echo GenerateKeyAndSave($_connection, $id);
  }

  function CheckUserExists($_connection, $userName, $password) {
      $user = $_connection->QueryParams("SELECT id, password FROM lmci_user
        where name = $1", array($userName))
        ->GetNext();

      if (!$user) {
        echo -2;
        exit();
      }

      $passwordHash = $user[1];
      $passwordsMatch = password_verify($password, $passwordHash);

      if (!$passwordsMatch) {
        echo -3;
        exit();
      }

      return $user[0];
  }

  function ClearOldKey($_connection, $id) {
    $_connection->Execute("DELETE FROM lmci_key WHERE lmci_user = $1", array($id));
  }

  function GenerateKeyAndSave($_connection, $id) {
    $key = md5(uniqid(rand(), true));
    $date = date('Y-m-d H:i:s');
    $expire = date('Y-m-d H:i:s', strtotime($date . " + 7 days"));
    $_connection->Execute("INSERT INTO lmci_key (key, lmci_user, expire) VALUES ($1, $2, $3)",
      array($key, $id, $expire));

    return $key;
  }

  function HashPassword($password) {
    return password_hash($password, PASSWORD_BCRYPT);
  }

  function GetFromPost($name) {
    if (!isset($_POST[$name])) {
      echo -1;
      exit();
    }

    return pg_escape_string($_POST[$name]);
  }
?>
