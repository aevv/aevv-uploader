<?php
  require_once("../../private/data-access/pgsql.php");
  require_once("../../private/secrets.php");

  $_pgConnection = new PGDataAccess(Secrets::$Host, Secrets::$Username, Secrets::$Password, Secrets::$Database);
  $_pgConnection->Connect();

  $id = GetIdFromURL();
  $image = GetImage($_pgConnection, $id);
  OutputImage($image);

  function GetImage($_connection, $id) {
    $imageRow = $_connection->Query(
    "SELECT data FROM LMCI_BLOB B INNER JOIN LMCI_UPLOAD U on U.id = B.lmci_upload
     WHERE U.id = $id")->GetNext();

     if (!$imageRow) {
       // TODO: actual 404;
       echo "404";
       exit();
     }

     return $imageRow[0];
  }

  function GetIdFromURL() {
    if (!isset($_GET["id"])) {
      // TODO: actual 404
      echo "404";
      exit();
    }

    return pg_escape_string($_GET["id"]);
  }

  function OutputImage($base64) {
    Header("Content-type: image/png");
    $data = str_replace("data:image/png;base64,", "", $base64);

    echo base64_decode($data);
  }
?>
