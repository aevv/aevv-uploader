<?php
  require_once("../../private/data-access/pgsql.php");
  require_once("../../private/secrets.php");

  $_pgConnection = new PGDataAccess(Secrets::$Host, Secrets::$Username, Secrets::$Password, Secrets::$Database);
  $_pgConnection->Connect();

  Process($_pgConnection);

  function Process($_connection) {
    $userName = GetFromPost("username");
    $email = GetFromPost("email");

    VerifyNewUser($_connection, $userName, $email);

    $password = GetFromPost("password");
    $hashedPassword = HashPassword($password);

    $id = SaveUser($_connection, $userName, $email, $hashedPassword);
    $key = GenerateKeyAndSave($_connection, $id);

    echo $key;
    exit();
  }

  function SaveUser($_connection, $userName, $email, $hashedPassword) {
    $id = $_connection->QueryParams("INSERT INTO lmci_user (name, email, password)
    VALUES ($1, $2, $3) RETURNING id", array($userName, $email, $hashedPassword))->GetNext()[0];

    return $id;
  }

  function GenerateKeyAndSave($_connection, $id) {
    $key = md5(uniqid(rand(), true));
    $date = date('Y-m-d H:i:s');
    $expire = date('Y-m-d H:i:s', strtotime($date . " + 7 days"));
    $_connection->Execute("INSERT INTO lmci_key (key, lmci_user, expire) VALUES ($1, $2, $3)",
      array($key, $id, $expire));

    return $key;
  }

  function GetFromPost($name) {
    if (!isset($_POST[$name])) {
      echo -1;
      exit();
    }

    return pg_escape_string($_POST[$name]);
  }

  function HashPassword($password) {
    return password_hash($password, PASSWORD_BCRYPT);
  }

  function VerifyNewUser($_connection, $userName, $email) {
    $userExists = $_connection->QueryParams("SELECT name FROM lmci_user
        WHERE name = $1 OR email = $2", array($userName, $email))->GetNext();

    if ($userExists) {
      echo -2;
      exit;
    }
  }
?>
