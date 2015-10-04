<?php
  require_once("../../private/data-access/pgsql.php");
  require_once("../../private/secrets.php");

  $_pgConnection = new PGDataAccess(Secrets::$Host, Secrets::$Username, Secrets::$Password, Secrets::$Database);
  $_pgConnection->Connect();

  $key = GetFromPost("key");

  Process($_pgConnection, $key);

  function Process($_connection, $key) {
    $expire = $_connection->QueryParams("SELECT expire FROM lmci_key where key = $1", array($key))
      ->GetNext()[0];
    $date = date("Y-m-d H:i:s");
    if ($expire <= $date) {
      echo 1;
      exit();
    }
    // TODO: Really need to come up with proper responses...
    echo 0;
  }

  function GetFromPost($name) {
    if (!isset($_POST[$name])) {
      echo -1;
      exit();
    }

    return pg_escape_string($_POST[$name]);
  }
?>
