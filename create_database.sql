CREATE DATABASE voteup WITH ENCODING 'UTF8';
CREATE USER voteup with encrypted password 'voteup123';
GRANT ALL privileges ON database voteup TO voteup;
\c voteup
GRANT ALL privileges ON schema public TO voteup;
