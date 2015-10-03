<?php
	class PGDataAccess {
		private $_host;
		private $_pass;
		private $_user;
		private $_db;

		private $_connection;

		public function __construct($host, $user, $pass, $db) {
			$this->_host = $host;
			$this->_pass = $pass;
			$this->_user = $user;
			$this->_db = $db;
		}

		public function Connect() {
			$this->_connection = pg_connect("host=$this->_host user=$this->_user password=$this->_pass dbname=$this->_db");
		}

		public function Query($sql) {
			return new PGReader(pg_query($this->_connection, $sql));
		}

		public function QueryParams($sql, $params) {
			return new PGReader(pg_query_params($this->_connection, $sql, $params));
		}

		public function Execute($sql, $params) {
			pg_prepare($this->_connection, "test", $sql);
			pg_execute($this->_connection, "test", $params);
		}

		public function Close() {
			pg_close($this->_connection);
		}

		public function GetError() {
			return pg_last_error($this->_connection);
		}
	}

	class PGReader {
		private $_rowSet;

		public function __construct($rowSet) {
			$this->_rowSet = $rowSet;
		}

		public function GetNext() {
			return pg_fetch_row($this->_rowSet);
		}
	}
?>
